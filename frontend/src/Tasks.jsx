import { useEffect, useState } from 'react';
import api from "./api/axios"
import './App.css'
import Button from "./components/Button";
import Modal from "./components/Modal";

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const [newTaskTitle, setNewTaskTitle] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState([]);
  const [createTask, setCreateTask] = useState([]);

  useEffect(() => {
    api.get('/api/tasks')
      .then(res => { setTasks(Array.isArray(res.data) ? res.data : []); })
      .catch(err => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  // useEffect(() => {
  //   let jsonString = "{task: {title:"+newTaskTitle+" }}";
  //   if(createTask){
  //     api.create('/api/tasks')
  //     .then(res => { setTasks(Array.isArray(res.data) ? res.data : []); })
  //     .catch(err => { setTasks([]); });
  //   }
  // }, []);

  return (
    <div className="app">
      <h1>📝 Task List</h1>
      <ul>
        {loading ? (
          <p>Loading...</p>
        ) : tasks.length === 0 ? (
          <p>No Tasks Found</p>
        ) : (
          tasks.map(task => (
            <div key={task.id}>{task.title}</div>
          ))
        )}
      </ul>
      <div className="p-4">
        <Button text="Create New Task" onClick={() => setIsModalOpen(true)} />
        {/* <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
          <h2 className="text-xl font-bold mb-4">Hello Modal!</h2>
          <p>This is the modal content.</p>
          <Button text="Close" onClick={() => setIsModalOpen(false)} className="mt-4 bg-blue-500 hover:bg-blue-600" />
          <Button text="Close" onClick={() => setIsModalOpen(false)} className="mt-4 bg-red-500 hover:bg-red-600" />
        </Modal> */}
      </div>
    </div>
  );
  
}

export default Tasks;
