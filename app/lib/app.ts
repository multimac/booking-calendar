export class App {
    public heading = "Todos";
    public description = '';

    public addTodo() {
        if(this.description) {
            console.log(this.description);
            this.description = '';
        }
    }
    public removeTodo(todo: any) {
        console.log(this.description);
    }
}