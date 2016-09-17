import { Router, RouterConfiguration } from 'aurelia-router';

export class App {
    public configureRouter(config: RouterConfiguration, router: Router): void {
        config.map([
            { route: "", moduleId: "./login", title: "Login" }
        ]);
    }
}