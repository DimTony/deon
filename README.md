import { useState, useCallback } from "react";

/**
 * Standard API Response structure for paginated data
 */
export interface PaginatedApiResponse<T> {
  data: T[];
  totalCount: number;
  totalPages: number;
  pageNumber: number;
  pageSize: number;
  hasNext: boolean;
  hasPrevious: boolean;
  success: boolean;
  message: string;
  errors: any;
}

/**
 * Payload structure for fetching paginated data
 */
export interface PaginatedRequestPayload {
  pageNumber: number;
  pageSize: number;
  filter?: Record<string, any>;
}

/**
 * Hook configuration options
 */
export interface UsePaginatedDataOptions<T, F = any> {
  pageSize?: number;
  initialFilters?: F;
  onSuccess?: (data: PaginatedApiResponse<T>) => void;
  onError?: (error: any) => void;
}

/**
 * Reusable Hook for Paginated Data
 *
 * This hook handles all the common state and logic for paginated API calls.
 *
 * @example
 * ```tsx
 * const {
 *   data,
 *   loading,
 *   currentPage,
 *   totalPages,
 *   totalCount,
 *   filters,
 *   setCurrentPage,
 *   setFilters,
 *   fetchData,
 *   refetch
 * } = usePaginatedData({
 *   fetchFn: fetchAllUsers,
 *   pageSize: 10,
 *   initialFilters: { status: "active" }
 * });
 *
 * // In useEffect
 * useEffect(() => {
 *   fetchData();
 * }, [currentPage]);
 * ```
 */
export function useTableData<T, F = any>(
  fetchFn: (
    payload: PaginatedRequestPayload
  ) => Promise<PaginatedApiResponse<T>>,
  options: UsePaginatedDataOptions<T, F> = {}
) {
  const {
    pageSize = 10,
    initialFilters = {} as F,
    onSuccess,
    onError,
  } = options;

  const [data, setData] = useState<T[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [filters, setFilters] = useState<F>(initialFilters);

  /**
   * Main fetch function
   */
  const fetchData = useCallback(
    async (customFilters?: F, customPage?: number) => {
      setLoading(true);
      try {
        const filtersToUse =
          customFilters !== undefined ? customFilters : filters;
        const pageToUse = customPage !== undefined ? customPage : currentPage;

        const payload: PaginatedRequestPayload = {
          pageNumber: pageToUse,
          pageSize: pageSize,
          filter: filtersToUse as Record<string, any>,
        };

        const response = await fetchFn(payload);

        setData(response.data || []);
        setTotalCount(response.totalCount || 0);
        setTotalPages(response.totalPages || 0);

        onSuccess?.(response);
      } catch (error) {
        console.error("Failed to fetch paginated data:", error);

        setData([]);
        setTotalCount(0);
        setTotalPages(0);

        onError?.(error);
      } finally {
        setLoading(false);
      }
    },
    [fetchFn, filters, currentPage, pageSize, onSuccess, onError]
  );

  /**
   * Update filters and reset to page 1
   */
  const updateFilters = useCallback(
    (newFilters: F | ((prev: F) => F)) => {
      const updatedFilters =
        typeof newFilters === "function" ? newFilters(filters) : newFilters;

      setFilters(updatedFilters);
      setCurrentPage(1);
      fetchData(updatedFilters, 1);
    },
    [filters, fetchData]
  );

  /**
   * Refetch current page with current filters
   */
  const refetch = useCallback(() => {
    fetchData(filters, currentPage);
  }, [fetchData, filters, currentPage]);

  /**
   * Go to specific page
   */
  const goToPage = useCallback(
    (page: number) => {
      if (page >= 1 && page <= totalPages) {
        setCurrentPage(page);
      }
    },
    [totalPages]
  );

  /**
   * Reset to initial state
   */
  const reset = useCallback(() => {
    setCurrentPage(1);
    setFilters(initialFilters);
    setData([]);
    setTotalCount(0);
    setTotalPages(0);
  }, [initialFilters]);

  return {
    // State
    data,
    loading,
    currentPage,
    totalPages,
    totalCount,
    filters,
    pageSize,

    // Setters
    setCurrentPage: goToPage,
    setFilters: updateFilters,

    // Actions
    fetchData,
    refetch,
    reset,
  };
}




This expression is not callable.
  Not all constituents of type '((prev: F) => F) | (F & Function)' are callable.
    Type 'F & Function' has no call signatures.ts(2349)
(parameter) newFilters: ((prev: F) => F) | (F & Function) 



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




