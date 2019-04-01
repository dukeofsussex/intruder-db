import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { AuthService } from './auth.service';

import { environment } from '../environments/environment';

@Injectable()
export class DataService {

    constructor(private authService: AuthService, private http: HttpClient) { }

    get(path: string, params: object = null, useAuth: boolean = false): any {
        return this.request('GET', path, params, useAuth);
    }

    post(path: string, body: object = null, useAuth: boolean = false): any {
        return this.request('POST', path, body, useAuth);
    }

    put(path: string, body: object = null): any {
        return this.request('PUT', path, body, true);
    }

    delete(path: string): any {
        return this.request('DELETE', path, null, true);
    }

    private request(method: string, path: string, params: any = null, useAuth: boolean = false): any {

        let headers = null;

        if (useAuth) {
            headers = new HttpHeaders({
                'Authorization': this.authService.getAuthorizationHeaderValue()
            });
        }

        return this.http.request(method, environment.backendURL + path, {
            body: (method !== 'GET') ? params : null,
            headers: headers,
            params: (method === 'GET') ? params : null,
        });
    }
}
