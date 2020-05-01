import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { WalletBalance, User } from '@app/models';
import { AuthenticationService } from './authentication.service';
import { environment } from '@environments/environment';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class BalanceService {
    private hubConnection: signalR.HubConnection;
    private currentBalanceSubject: BehaviorSubject<WalletBalance>;
    public currentBalance: Observable<WalletBalance>;

    private hubUrl = `${environment.apiUrl}/hubs/balance`;
    private apiUrl = `${environment.apiUrl}/UserBalances`;

    constructor(
        private authenticationService: AuthenticationService,
        private http: HttpClient
    ) {
        this.authenticationService.currentUser.subscribe(x => this.connectToBallanceHub(x));
        this.currentBalanceSubject = new BehaviorSubject<WalletBalance>(new WalletBalance());
        this.currentBalance = this.currentBalanceSubject.asObservable();
        if (this.authenticationService.currentUserValue) {
            this.getBallance().subscribe();
        }
    }

    public get currentBalanceValue(): WalletBalance {
        return this.currentBalanceSubject.value;
    }

    private handleError(error: any) {
        console.log(error);
        return throwError(error);
    }

    private connectToBallanceHub(currentUser: User) {
        let token = '';

        if (currentUser) {
            token = currentUser.token;

            if (this.hubConnection) {
                this.hubConnection.stop();
            }

            this.hubConnection = new signalR.HubConnectionBuilder()
                                            .withUrl(this.hubUrl, { accessTokenFactory: () => token })
                                            .build();
            this.hubConnection
                .start()
                .then(() => console.log('Connection started'))
                .catch(err => console.log('Error while starting connection: ' + err));

            this.hubConnection.on('UpdateBalance', data => this.currentBalanceSubject.next(data));

            this.getBallance().subscribe();
        }
    }

    public logout() {
        if (this.hubConnection) {
            this.hubConnection.stop();
        }
    }

    public getBallance(): Observable<WalletBalance> {
        return this.http
                   .get<WalletBalance>(`${this.apiUrl}/GetBalance`)
                   .pipe(
                        tap(data => this.currentBalanceSubject.next(data)),
                        catchError(this.handleError)
                   );
    }
}
