import './App.css'
import api from "./api/axios"
import { useEffect, useState } from 'react';
import Tasks from "./Tasks"
function App() {
  const [email, setEmail] = useState([]);
  const [password, setPassword] = useState([]);
  const [users, setUsers] = useState([]);

  useEffect(() => {
    
    api.get('/api/Users')
      .then(res => setUsers(res.data))
      .catch(err => console.error(err));
  }, []);


  return (
    <div className="app">
      <h1>📝 React Task Evaluator</h1>
      <Tasks/>

    </div>
  );
}

export default App
