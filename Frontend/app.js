const API_URL = "http://localhost:5000/api/todos";

const form = document.getElementById("todo-form");
const input = document.getElementById("todo-input");
const list = document.getElementById("todo-list");

// Pobranie i wyswietlenie wszystkich zadan
async function loadTodos() {
    const response = await fetch(API_URL);
    const todos = await response.json();

    list.innerHTML = "";
    todos.forEach(todo => {
        const li = document.createElement("li");
        if (todo.isCompleted) li.classList.add("completed");

        const span = document.createElement("span");
        span.textContent = todo.title;
        span.onclick = () => toggleTodo(todo);

        const btn = document.createElement("button");
        btn.textContent = "Usun";
        btn.onclick = () => deleteTodo(todo.id);

        li.appendChild(span);
        li.appendChild(btn);
        list.appendChild(li);
    });
}

// Dodanie nowego zadania
form.addEventListener("submit", async (e) => {
    e.preventDefault();
    const title = input.value.trim();
    if (!title) return;

    await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title, isCompleted: false })
    });

    input.value = "";
    loadTodos();
});

// Zmiana statusu zadania
async function toggleTodo(todo) {
    await fetch(`${API_URL}/${todo.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            id: todo.id,
            title: todo.title,
            isCompleted: !todo.isCompleted
        })
    });
    loadTodos();
}

// Usuniecie zadania
async function deleteTodo(id) {
    await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    loadTodos();
}

// Zaladuj zadania przy starcie
loadTodos();
