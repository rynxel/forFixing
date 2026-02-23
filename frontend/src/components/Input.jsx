import React from "react";

const Input = ({ label, error, className = "", ...props }) => {
  return (
    <div className={`flex flex-col gap-1 w-full ${className}`}>
      {label && <label className="text-sm font-medium text-gray-700">{label}</label>}

      <input
        className="border rounded px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500"
        {...props}
      />

      {error && <span className="text-sm text-red-500">{error}</span>}
    </div>
  );
};

export default Input;