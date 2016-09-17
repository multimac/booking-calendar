import { inject } from "aurelia-framework";

import { LoginService } from "./services/http/login";

@inject(LoginService)
export class Login {
    public email = "";
    public password = "";

    constructor(private loginService: LoginService)
    { }

    public performLogin() {
        this.loginService.login(this.email, this.password);
    }
}