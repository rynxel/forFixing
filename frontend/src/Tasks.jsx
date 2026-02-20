import { useEffect, useState } from 'react';
import api from "./api/axios"

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const [newTaskTitle, setNewTaskTitle] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState([]);

  useEffect(() => {
    api.get('/tasks')
    .then(res => { setTasks(Array.isArray(res.data) ? res.data : []); })
    .catch(err => { setTasks([]); });
  }, []);

  useEffect(() => {
    let jsonString = "{task: {title:"+newTaskTitle+" }}";
    if(createTask){
      api.create('/tasks')
      .then(res => { setTasks(Array.isArray(res.data) ? res.data : []); })
      .catch(err => { setTasks([]); });
    }
  }, []);

  return (
    <div>
      <h2>Tasks</h2>
      <ul>
        {tasks && tasks.length > 0 ? (
          tasks.map(task => (
            <li key={task.id}>
              {task.title} {task.isDone ? '✅' : '❌'}
            </li>
          ))
        ) : (
          <li>No tasks yet</li>
        )}
      </ul>
      <div className="p-4">
        <Button text="Create New Task" onClick={() => setIsModalOpen(true)} />

        <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
          <h2 className="text-xl font-bold mb-4">Hello Modal!</h2>
          <p>This is the modal content.</p>
          <Button text="Close" onClick={() => setIsModalOpen(false)} className="mt-4 bg-blue-500 hover:bg-blue-600" />
          <Button text="Close" onClick={() => setIsModalOpen(false)} className="mt-4 bg-red-500 hover:bg-red-600" />
        </Modal>
      </div>
    </div>
  );
  
}

export default Tasks;
