// components/Button.jsx
import React from 'react';

export default function Button({ text, onClick, type = 'button', className = '', disabled = false }) {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled}
      className={`px-4 py-2 rounded bg-blue-600 text-white hover:bg-blue-700 transition-colors ${className}`}
    >
      {text}
    </button>
  );
}
