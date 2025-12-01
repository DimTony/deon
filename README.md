


"use client";

import { useUserService } from "@/hooks/useUserService";
import { usePaginatedData } from "@/hooks/usePaginatedData";
import { PaginatedTable } from "@/components/PaginatedTable";
import { PaginationControls } from "@/components/PaginationControls";
import { Eye, Plus, Search, SquarePen, Trash2 } from "lucide-react";
import React, { useEffect } from "react";

interface UserFilters {
  status: string;
  startDate: string;
  endDate: string;
  tab: "Pending" | "Claimed";
  globalSearch: string;
}

interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  status?: string;
  isActive?: boolean;
}

/**
 * Example: UserManagement Component using Reusable Components
 * 
 * This demonstrates the clean, reusable pattern for handling paginated data.
 */
const UserManagementReusable = () => {
  const { fetchAllUsers } = useUserService();

  // Use the reusable paginated data hook
  const {
    data: users,
    loading,
    currentPage,
    totalPages,
    totalCount,
    filters,
    pageSize,
    setCurrentPage,
    setFilters,
    fetchData,
    refetch,
  } = usePaginatedData<User, UserFilters>(fetchAllUsers, {
    pageSize: 10,
    initialFilters: {
      status: "",
      startDate: "",
      endDate: "",
      tab: "Pending",
      globalSearch: "",
    },
  });

  // Fetch data when component mounts or page changes
  useEffect(() => {
    fetchData();
  }, [currentPage]);

  // Handle search
  const handleSearch = (searchValue: string) => {
    setFilters((prev) => ({ ...prev, globalSearch: searchValue }));
  };

  // Define table columns using the reusable pattern
  const columns = [
    {
      key: "name",
      header: "Name",
      render: (user: User) => (
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-full bg-gradient-to-br from-blue-400 to-purple-500 flex items-center justify-center text-white font-semibold text-xs">
            {user.firstName?.[0]}{user.lastName?.[0]}
          </div>
          <div className="flex flex-col">
            <span>{user.firstName} {user.lastName}</span>
          </div>
        </div>
      ),
    },
    {
      key: "email",
      header: "Email",
      render: (user: User) => user.email,
    },
    {
      key: "role",
      header: "Role",
      render: (user: User) => user.role,
    },
    {
      key: "status",
      header: "Status",
      render: (user: User) => (
        <span
          className={`px-2 py-1 text-xs rounded-full ${
            user.status?.toLowerCase() === "active" || user.isActive
              ? "bg-green-600/40"
              : "bg-red-600/40"
          }`}
        >
          {user.status || (user.isActive ? "Active" : "Inactive")}
        </span>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      className: "py-3 px-2 text-right",
      render: (user: User) => (
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
      ),
    },
  ];

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

          {/* Reusable Table Component */}
          <div className="mt-6 backdrop-blur-xl bg-white/10 rounded-3xl p-4 border border-white/20 shadow-2xl">
            <PaginatedTable
              data={users}
              columns={columns}
              loading={loading}
              emptyMessage="No users found"
            />

            {/* Reusable Pagination Component */}
            <PaginationControls
              currentPage={currentPage}
              totalPages={totalPages}
              totalCount={totalCount}
              pageSize={pageSize}
              onPageChange={setCurrentPage}
              itemName="users"
            />
          </div>
        </section>
      </div>
    </div>
  );
};

export default UserManagementReusable;




