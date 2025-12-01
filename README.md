"use client";

import { useUserService } from "@/hooks/useUserService";
import { Eye, Plus, Search, SquarePen, Trash2 } from "lucide-react";
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
          startDate: "",
          endDate: "",
          globalSearch: filtersToUse.globalSearch,
          status: filtersToUse.status,
          tab: filtersToUse.tab,
        },
      };

      const data = await fetchAllUsers(payload);

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

  const handleSearch = (searchValue: string) => {
    const newFilters = { ...filters, globalSearch: searchValue };
    setFilters(newFilters);
    setCurrentPage(1); // Reset to first page on search
    fetchTableData(newFilters, 1);
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
                  value={filters.globalSearch}
                  onChange={(e) => handleSearch(e.target.value)}
                  className="w-40 pl-7 pr-2 py-1 border border-gray-300 rounded-md text-sm text-black
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
                {loading ? (
                  <tr>
                    <td colSpan={5} className="py-8 text-center text-gray-400">
                      <div className="flex items-center justify-center gap-2">
                        <div className="w-4 h-4 border-2 border-white/20 border-t-white rounded-full animate-spin" />
                        Loading users...
                      </div>
                    </td>
                  </tr>
                ) : tableData.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="py-8 text-center text-gray-400">
                      No users found
                    </td>
                  </tr>
                ) : (
                  tableData.map((user, index) => (
                    <tr
                      key={user.id || index}
                      className="border-b border-white/10 hover:bg-white/5 transition"
                    >
                      <td className="py-2 px-2">
                        <div className="flex items-center gap-2">
                          <div className="w-8 h-8 rounded-full bg-gradient-to-br from-blue-400 to-purple-500 flex items-center justify-center text-white font-semibold text-xs">
                            {user.firstName?.[0]}{user.lastName?.[0]}
                          </div>
                          <div className="flex flex-col">
                            <span>{user.firstName} {user.lastName}</span>
                          </div>
                        </div>
                      </td>
                      <td className="py-2 px-2">{user.email}</td>
                      <td className="py-2 px-2">{user.role}</td>
                      <td className="py-2 px-2">
                        <span
                          className={`px-2 py-1 text-xs rounded-full ${
                            user.status?.toLowerCase() === "active" ||
                            user.isActive
                              ? "bg-green-600/40"
                              : "bg-red-600/40"
                          }`}
                        >
                          {user.status || (user.isActive ? "Active" : "Inactive")}
                        </span>
                      </td>
                      <td className="text-right px-2">
                        <div className="flex items-center justify-end gap-2">
                          <button
                            className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/65 rounded-full p-1.5 cursor-pointer text-blue-300 hover:text-blue-400 text-xs transition-all duration-300"
                            title="View"
                            onClick={() => console.log("View user:", user.id)}
                          >
                            <Eye size={12} />
                          </button>
                          <button
                            className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/65 rounded-full p-1.5 cursor-pointer text-blue-300 hover:text-blue-400 text-xs transition-all duration-300"
                            title="Edit"
                            onClick={() => console.log("Edit user:", user.id)}
                          >
                            <SquarePen size={12} />
                          </button>
                          <button
                            className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/65 rounded-full p-1.5 cursor-pointer text-red-300 hover:text-red-400 text-xs transition-all duration-300"
                            title="Delete"
                            onClick={() => console.log("Delete user:", user.id)}
                          >
                            <Trash2 size={12} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>

            {/* Pagination Controls */}
            {!loading && tableData.length > 0 && (
              <div className="mt-4 flex items-center justify-between text-sm">
                <div className="text-gray-300">
                  Showing {((currentPage - 1) * pageSize) + 1} to{" "}
                  {Math.min(currentPage * pageSize, totalCount)} of {totalCount}{" "}
                  users
                </div>

                <div className="flex items-center gap-2">
                  <button
                    onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
                    disabled={currentPage === 1}
                    className="px-3 py-1 backdrop-blur-xl bg-white/10 text-white rounded-md text-xs hover:bg-white/65 hover:text-black cursor-pointer transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:bg-white/10 disabled:hover:text-white"
                  >
                    Previous
                  </button>

                  <div className="flex items-center gap-1">
                    {Array.from({ length: totalPages }, (_, i) => i + 1).map(
                      (page) => (
                        <button
                          key={page}
                          onClick={() => setCurrentPage(page)}
                          className={`px-2 py-1 rounded-md text-xs transition-all duration-300 ${
                            currentPage === page
                              ? "bg-white text-black"
                              : "backdrop-blur-xl bg-white/10 text-white hover:bg-white/65 hover:text-black"
                          }`}
                        >
                          {page}
                        </button>
                      )
                    )}
                  </div>

                  <button
                    onClick={() =>
                      setCurrentPage((prev) => Math.min(totalPages, prev + 1))
                    }
                    disabled={currentPage === totalPages}
                    className="px-3 py-1 backdrop-blur-xl bg-white/10 text-white rounded-md text-xs hover:bg-white/65 hover:text-black cursor-pointer transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:bg-white/10 disabled:hover:text-white"
                  >
                    Next
                  </button>
                </div>
              </div>
            )}
          </div>
        </section>
      </div>
    </div>
  );
};

export default UserManagement;
