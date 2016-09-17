import { HttpClient, json } from "aurelia-fetch-client";
import { inject } from "aurelia-framework";

import { BaseHttpService } from "./base";

/**
 * Handles logging a user in or out, and checking if they are already logged in.
 */
@inject(HttpClient)
export class LoginService extends BaseHttpService {
    constructor(client: HttpClient) {
        super(client);
    }

    /**
     * Attempts to log a user with the given email and password in.
     * @param email The email of the user to log in.
     * @param password The password for the given user.
     */
    public login(email: string, password: string): void {
        this.client.fetch("account/login", {
            body: json({ email: email, password: password }),
            method: "post"
        });
    }

    /**
     * Logs a user out, regardless of if they're logged in or not.
     */
    public logout(): void {
        this.client.fetch("account/logout", { method: "post" });
    }

    /**
     * Pings the server to see if this client is authenticated.
     */
    public ping(): void {
        this.client.fetch("account/ping", { method: "post" });
    }
}