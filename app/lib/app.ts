import { Todo } from "./todo";

export class App {
    public heading = "Todos";
    public description = '';

    public todos: Todo[] = [];

    public addTodo() {
        if (this.description) {
            this.todos.push(new Todo(this.description));
            this.description = '';
        }
    }
    public removeTodo(todo: any) {
        const index = this.todos.indexOf(todo);
        if (index !== -1) {
            this.todos.splice(index, 1);
        }
    }
}