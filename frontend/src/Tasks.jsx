import { useEffect, useState } from 'react';
import api from "./api/axios"

function Tasks() {
  const [tasks, setTasks] = useState([]);
  const [newTaskTitle, setNewTaskTitle] = useState([]);
  const [newTask, setNewTask] = useState([]);

  useEffect(() => {
    api.get('/tasks')
    .then(res => { setTasks(Array.isArray(res.data) ? res.data : []); })
    .catch(err => { setTasks([]); });
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
    </div>
  );
  
}

export default Tasks;
