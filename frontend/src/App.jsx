import './App.css'
import api from "./api/axios"
import { useEffect, useState } from 'react';
import Button from "./components/Button";
import Input from "./components/Input";
function App() {
  const [email, setEmail] = useState("");       
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState([]);
  const handleLogin = async () => {
    setLoading(true);
    setError(""); // clear previous errors
  
    try {
      const res = await api.post("/api/auth/login", {
        email,
        password,
      });
  
      // Login successful
      console.log("Success:", res.data);
  
      // Example: store tokens if returned
      const { accessToken, refreshToken } = res.data;
      if (accessToken) localStorage.setItem("accessToken", accessToken);
      if (refreshToken) localStorage.setItem("refreshToken", refreshToken);
  
      // Redirect or update UI
    } catch (err) {
      // Axios error response
      if (err.response) {
        // Backend responded with status code
        if (err.response.status === 401) {
          setError(err.response.data.message || "Invalid credentials");
        } else {
          setError(`Error: ${err.response.statusText}`);
        }
      } else if (err.request) {
        // Request was made but no response
        setError("No response from server");
      } else {
        // Other errors
        setError(err.message);
      }
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="app">
        <h1>Login</h1>
        {error && <p className="text-red-500">{error}</p>}
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleLogin();
          }}
        >
        <div className="flex flex-col gap-4 w-80">
          <Input
            label="Email"
            type="email"
            value={email}
            autoComplete="current-password"
            onChange={(e) => setEmail(e.target.value)}
          />
          <Input
            label="Password"
            type="password"
            value={password}
            autoComplete="current-password"
            onChange={(e) => setPassword(e.target.value)}
          />
          <button
            onClick={handleLogin}
            disabled={loading}
            className="bg-blue-500 text-white px-4 py-2 rounded disabled:opacity-50"
          >
          {loading ? "Logging in..." : "Login"}
          </button>
        </div>
        </form>
        
    </div>
  );
}

export default App
