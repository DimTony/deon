"use client";

import { useUserService } from "@/hooks/useUserService";
import { Eye, Plus, Search, SquarePen, Trash2, View } from "lucide-react";
import React, { useEffect, useState } from "react";

interface Filters {
  status: string;
  startDate: string;
  endDate: string;
  tab: "Pending" | "Claimed";
  globalSearch: string;
}

const UserManagement = () => {
  const { fetchAllUsers } = useUserService();
  const pageSize = 10;
  const [tableData, setTableData] = useState<any[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [filters, setFilters] = useState<Filters>({
    status: "",
    startDate: "",
    endDate: "",
    tab: "Pending",
    globalSearch: "",
  });

  useEffect(() => {
    fetchTableData();
  }, [currentPage]);

  const fetchTableData = async (
    customFilters?: Filters,
    customPage?: number
  ) => {
    setLoading(true);
    try {
      const filtersToUse = customFilters || filters;
      const pageToUse = customPage || currentPage;

      const payload = {
        pageNumber: pageToUse,
        pageSize: pageSize,
        filter: {
          date: "",
          // startDate: formatDateForAPI(filtersToUse.startDate),
          // endDate: formatDateForAPI(filtersToUse.endDate),
          startDate: "",
          endDate: "",
          globalSearch: filtersToUse.globalSearch,
          status: filtersToUse.status,
          tab: filtersToUse.tab,
        },
      };

      const data = await fetchAllUsers(payload);

      // console.log("FETCHED DATAAA:", data);

      setTableData(data.data || []);
      setTotalCount(data.totalCount);
      setTotalPages(data.totalPages);
    } catch (error) {
      console.error("Failed to fetch table data:", error);

      setTableData([]);
      setTotalCount(0);
      setTotalPages(0);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen h-full w-full bg-[url('/images/home.jpg')] bg-cover bg-center pt-10 px-6">
      {/* Overlay */}
      <div className="absolute inset-0 bg-black/70"></div>

      <div className="relative z-10 text-white py-6">
        <section>
          <div className="relative flex justify-between backdrop-blur-xl bg-white/10 rounded-3xl p-4 border border-white/20 shadow-2xl">
            <span>User Management</span>

            <div className="flex items-center gap-3">
              <div className="relative">
                <Search
                  size={14}
                  className="absolute left-2 top-1/2 -translate-y-1/2 text-gray-400"
                />
                <input
                  type="text"
                  placeholder="Search users..."
                  className="w-40 pl-7 pr-2 py-1 border border-gray-300 rounded-md text-sm 
                 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>

              <button className="flex items-center gap-1 px-2 py-1 backdrop-blur-xl bg-white/10 text-white rounded-md text-xs hover:bg-white/65 hover:text-black cursor-pointer transition-all duration-300">
                <Plus size={14} />
                Add User
              </button>
            </div>
          </div>

          {/* Users Table */}
          <div className="mt-6 backdrop-blur-xl bg-white/10 rounded-3xl p-4 border border-white/20 shadow-2xl overflow-x-auto">
            <table className="w-full text-left text-sm text-white">
              <thead>
                <tr className="border-b border-white/20 text-gray-200">
                  <th className="py-3 px-2">Name</th>
                  <th className="py-3 px-2">Email</th>
                  <th className="py-3 px-2">Role</th>
                  <th className="py-3 px-2">Status</th>
                  <th className="py-3 px-2 text-right">Actions</th>
                </tr>
              </thead>

              <tbody>
                <tr className="border-b border-white/10 hover:bg-white/5 transition">
                  {/* <td className="py-2 px-2">John Doe</td> */}
                  <td className="py-2 px-2">
                    <div className="flex items-center gap-1">
                      <div className="w-8 h-8 rounded-full bg-red-200" />
                      <div className="flex flex-col">
                        <span>John Doe</span>
                        {/* <span className="text-[9px]">Jjohn@example.com</span> */}
                      </div>
                    </div>
                  </td>
                  <td className="py-2 px-2">john@example.com</td>
                  <td className="py-2 px-2">Admin</td>
                  <td className="py-2 px-2">
                    <span className="px-2 py-1 text-xs bg-green-600/40 rounded-full">
                      Active
                    </span>
                  </td>
                  <td className="text-right px-2">
                    <div className="flex items-center justify-end gap-2">
                      <button className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/65 rounded-full p-1.5 cursor-pointer  text-blue-300 hover:text-blue-400 text-xs">
                        <Eye size={12} />
                      </button>
                      <button className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/65 rounded-full p-1.5 cursor-pointer  text-blue-300 hover:text-blue-400 text-xs">
                        <SquarePen size={12} />
                      </button>
                      <button className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/65 rounded-full p-1.5 cursor-pointer  text-blue-300 hover:text-blue-400 text-xs">
                        <Trash2 size={12} />
                      </button>
                    </div>
                  </td>
                </tr>

                <tr className="border-b border-white/10 hover:bg-white/5 transition">
                  <td className="py-2 px-2">Jane Smith</td>
                  <td className="py-2 px-2">jane@example.com</td>
                  <td className="py-2 px-2">User</td>
                  <td className="py-2 px-2">
                    <span className="px-2 py-1 text-xs bg-red-600/40 rounded-full">
                      Inactive
                    </span>
                  </td>
                  <td className="py-2 px-2 text-right">
                    <button className="text-blue-300 hover:text-blue-400 text-xs">
                      Edit
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </div>
  );
};

export default UserManagement;

data
: 
Array(2)
0
: 
{id: 1003, email: 'allenbrownkane@gmail.com', firstName: 'Allen', lastName: 'Kane', role: 'Manager', …}
1
: 
{id: 3, email: 'tonystoryemail@gmail.com', firstName: 'Tony', lastName: 'Story', role: 'Admin', …}
length
: 
2
[[Prototype]]
: 
Array(0)
errors
: 
null
hasNext
: 
false
hasPrevious
: 
false
message
: 
"Retrieved 2 of 2 users"
pageNumber
: 
1
pageSize
: 
10
success
: 
true
totalCount
: 
2
totalPages
: 
1


