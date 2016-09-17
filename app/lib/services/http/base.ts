import { HttpClient } from "aurelia-fetch-client";

/**
 * Handles configuring default values for the HttpClient required by api.calend.ar.
 * These include headers (like 'X-Requested-With'), the baseUrl, and default error handling.
 */
export abstract class BaseHttpService {
    /**
     * @param client An instance of HttpClient to configure and use.
     */
    constructor(protected client: HttpClient) {
        const antiforgeryName = document.body.dataset["antiforgeryName"];
        const antiforgeryValue = document.body.dataset["antiforgeryValue"];

        client.configure(config => {
            const headers: { [name: string]: string; } = {};
            headers[antiforgeryName] = antiforgeryValue;
            headers["X-Requested-With"] = "XMLHttpRequest";

            config.withBaseUrl("http://api.calend.ar/")
                .withDefaults({ headers: headers })
                .rejectErrorResponses();
        });
    }
}