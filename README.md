"use client";

import { X } from "lucide-react";
import React, { useEffect } from "react";

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
}

export const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  title,
  children,
}) => {
  // Close modal on ESC key press
  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    
    if (isOpen) {
      document.addEventListener("keydown", handleEsc);
      // Prevent body scroll when modal is open
      document.body.style.overflow = "hidden";
    }
    
    return () => {
      document.removeEventListener("keydown", handleEsc);
      document.body.style.overflow = "unset";
    };
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/60 backdrop-blur-sm"
        onClick={onClose}
      />

      {/* Modal Content */}
      <div className="relative z-10 w-full max-w-md mx-4 backdrop-blur-xl bg-white/10 rounded-2xl border border-white/20 shadow-2xl">
        {/* Header */}
        <div className="flex items-center justify-between p-4 border-b border-white/20">
          <h2 className="text-lg font-semibold text-white">{title}</h2>
          <button
            onClick={onClose}
            className="flex items-center justify-center backdrop-blur-xl bg-white/10 hover:bg-white/20 rounded-full p-1.5 cursor-pointer text-gray-300 hover:text-white transition-all duration-300"
          >
            <X size={18} />
          </button>
        </div>

        {/* Body */}
        <div className="p-4">{children}</div>
      </div>
    </div>
  );
};










"use client";

import React, { useEffect, useState } from "react";
import { Modal } from "./Modal";

interface User {
  id?: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  status?: string;
  isActive?: boolean;
}

interface UserFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (user: Partial<User>) => void | Promise<void>;
  user?: User | null;
  mode: "create" | "edit";
}

export const UserFormModal: React.FC<UserFormModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  user,
  mode,
}) => {
  const [formData, setFormData] = useState<Partial<User>>({
    firstName: "",
    lastName: "",
    email: "",
    role: "",
    status: "Active",
  });
  
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Populate form when editing
  useEffect(() => {
    if (mode === "edit" && user) {
      setFormData({
        firstName: user.firstName || "",
        lastName: user.lastName || "",
        email: user.email || "",
        role: user.role || "",
        status: user.status || (user.isActive ? "Active" : "Inactive"),
      });
    } else if (mode === "create") {
      setFormData({
        firstName: "",
        lastName: "",
        email: "",
        role: "",
        status: "Active",
      });
    }
    setErrors({});
  }, [mode, user, isOpen]);

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    // Clear error for this field
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.firstName?.trim()) {
      newErrors.firstName = "First name is required";
    }
    if (!formData.lastName?.trim()) {
      newErrors.lastName = "Last name is required";
    }
    if (!formData.email?.trim()) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Email is invalid";
    }
    if (!formData.role?.trim()) {
      newErrors.role = "Role is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    setIsSubmitting(true);
    try {
      const submitData = mode === "edit" && user ? { ...formData, id: user.id } : formData;
      await onSubmit(submitData);
      onClose();
    } catch (error) {
      console.error("Error submitting form:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={mode === "create" ? "Add New User" : "Edit User"}
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* First Name */}
        <div>
          <label className="block text-sm font-medium text-gray-200 mb-1">
            First Name <span className="text-red-400">*</span>
          </label>
          <input
            type="text"
            name="firstName"
            value={formData.firstName}
            onChange={handleInputChange}
            className={`w-full px-3 py-2 bg-white/10 border ${
              errors.firstName ? "border-red-500" : "border-white/20"
            } rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500`}
            placeholder="Enter first name"
          />
          {errors.firstName && (
            <p className="mt-1 text-xs text-red-400">{errors.firstName}</p>
          )}
        </div>

        {/* Last Name */}
        <div>
          <label className="block text-sm font-medium text-gray-200 mb-1">
            Last Name <span className="text-red-400">*</span>
          </label>
          <input
            type="text"
            name="lastName"
            value={formData.lastName}
            onChange={handleInputChange}
            className={`w-full px-3 py-2 bg-white/10 border ${
              errors.lastName ? "border-red-500" : "border-white/20"
            } rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500`}
            placeholder="Enter last name"
          />
          {errors.lastName && (
            <p className="mt-1 text-xs text-red-400">{errors.lastName}</p>
          )}
        </div>

        {/* Email */}
        <div>
          <label className="block text-sm font-medium text-gray-200 mb-1">
            Email <span className="text-red-400">*</span>
          </label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleInputChange}
            className={`w-full px-3 py-2 bg-white/10 border ${
              errors.email ? "border-red-500" : "border-white/20"
            } rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500`}
            placeholder="Enter email address"
          />
          {errors.email && (
            <p className="mt-1 text-xs text-red-400">{errors.email}</p>
          )}
        </div>

        {/* Role */}
        <div>
          <label className="block text-sm font-medium text-gray-200 mb-1">
            Role <span className="text-red-400">*</span>
          </label>
          <select
            name="role"
            value={formData.role}
            onChange={handleInputChange}
            className={`w-full px-3 py-2 bg-white/10 border ${
              errors.role ? "border-red-500" : "border-white/20"
            } rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500`}
          >
            <option value="" className="bg-gray-800">
              Select a role
            </option>
            <option value="Admin" className="bg-gray-800">
              Admin
            </option>
            <option value="Manager" className="bg-gray-800">
              Manager
            </option>
            <option value="User" className="bg-gray-800">
              User
            </option>
            <option value="Guest" className="bg-gray-800">
              Guest
            </option>
          </select>
          {errors.role && (
            <p className="mt-1 text-xs text-red-400">{errors.role}</p>
          )}
        </div>

        {/* Status */}
        <div>
          <label className="block text-sm font-medium text-gray-200 mb-1">
            Status
          </label>
          <select
            name="status"
            value={formData.status}
            onChange={handleInputChange}
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="Active" className="bg-gray-800">
              Active
            </option>
            <option value="Inactive" className="bg-gray-800">
              Inactive
            </option>
          </select>
        </div>

        {/* Action Buttons */}
        <div className="flex items-center justify-end gap-3 pt-4 border-t border-white/20">
          <button
            type="button"
            onClick={onClose}
            disabled={isSubmitting}
            className="px-4 py-2 text-sm font-medium text-gray-300 hover:text-white bg-white/10 hover:bg-white/20 rounded-lg transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            className="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSubmitting
              ? "Saving..."
              : mode === "create"
              ? "Add User"
              : "Save Changes"}
          </button>
        </div>
      </form>
    </Modal>
  );
};
