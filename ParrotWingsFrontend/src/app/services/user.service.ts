import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { User } from '@app/models';
import { Observable, throwError, of } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    apiUrl = `${environment.apiUrl}/users`;
    userNamesCache: Map<string, User>;

    constructor(private http: HttpClient) {
      this.userNamesCache = new Map<string, User>();
    }

    private handleError(error: any) {
        console.log(error);
        return throwError(error);
    }

    getUsers(): Observable<User[]> {
        return this.http.get<User[]>(`${this.apiUrl}/GetUsers`)
            .pipe(
                tap(data => {
                    for (const el of data) {
                        this.userNamesCache.set(el.id, el) ;
                    }
                }),
                catchError(this.handleError)
            );
    }

    getUser(id: string): Observable<User> {
        if (this.userNamesCache.has(id)) {
            return of(this.userNamesCache.get(id));
        } else {

            return this.http.get<User>(`${this.apiUrl}/${id}`)
                .pipe(
                    tap(data => {
                        this.userNamesCache.set(data.id, data);
                    }),
                    catchError(this.handleError)
                );
        }
    }
}
