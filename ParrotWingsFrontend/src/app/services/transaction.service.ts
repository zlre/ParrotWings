import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { User, Transaction } from '@app/models';
import { AuthenticationService } from './authentication.service';
import { environment } from '@environments/environment';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Injectable({
    providedIn: 'root'
})
export class TransactionService {
    private hubConnection: signalR.HubConnection;
    private currentTransacionsSubject: BehaviorSubject<Transaction[]>;
    public currentTransacions: Observable<Transaction[]>;
    private exampleTransactionSubject: BehaviorSubject<Transaction>;
    public exampleTransaction: Observable<Transaction>;
    private COUNT = 20;

    private hubUrl = `${environment.apiUrl}/hubs/transactions`;
    private apiUrl = `${environment.apiUrl}/Transactions`;

    constructor(
        private authenticationService: AuthenticationService,
        private http: HttpClient,
        private toastr: ToastrService
    ) {
        this.authenticationService.currentUser.subscribe(x => this.connectToBallanceHub(x));

        this.currentTransacionsSubject = new BehaviorSubject<Transaction[]>([]);
        this.currentTransacions = this.currentTransacionsSubject.asObservable();

        this.exampleTransactionSubject = new BehaviorSubject<Transaction>(new Transaction());
        this.exampleTransaction = this.exampleTransactionSubject.asObservable();

        if (this.authenticationService.currentUserValue) {
            this.getRecentTransactions(this.COUNT).subscribe();
        }
    }

    public setExampleTransaction(transaction: Transaction){
        this.exampleTransactionSubject.next(transaction);
    }

    public get currentTransacionsValue(): Transaction[] {
        return this.currentTransacionsSubject.value;
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

            this.hubConnection.on('UpdateTransaction', data => {
                if (data.status === 1) {
                    this.toastr.success('Transaction successfully confirmed', 'Success');
                }
                if (data.status === 2) {
                    this.toastr.error('Transaction rejected', 'Error');
                }

                let result = this.currentTransacionsValue.filter(f => f.id !== data.id);
                result = [data, ...result];
                this.currentTransacionsSubject.next(result);
            });

            this.hubConnection.on('NewTransaction', data => {
                this.toastr.success('Transaction successfully created', 'Success');
                const result = [data, ...this.currentTransacionsValue];
                this.currentTransacionsSubject.next(result);
            });

            this.getRecentTransactions(this.COUNT).subscribe();
        }
    }

    public logout() {
        if (this.hubConnection) {
            this.hubConnection.stop();
        }
    }

    public getRecentTransactions(count: number): Observable<Transaction[]> {
        return this.http
                   .get<Transaction[]>(`${this.apiUrl}/GetUserRecentTransactions/?count=${count}`)
                   .pipe(
                        tap(data => this.currentTransacionsSubject.next(data)),
                        catchError(this.handleError)
                   );
    }

    public createTransaction(transaction: Transaction): Observable<Transaction> {
        return this.http.post<Transaction>(`${this.apiUrl}/CreateTransaction`, transaction)
            .pipe(
                catchError(this.handleError)
            );
    }

    public commitTransaction(transaction: Transaction): Observable<Transaction> {
        return this.http.post<Transaction>(`${this.apiUrl}/CommitTransaction`, transaction)
            .pipe(
                catchError(this.handleError)
            );
    }

    public rejectTransaction(transaction: Transaction): Observable<Transaction> {
        return this.http.post<Transaction>(`${this.apiUrl}/RejectTransaction`, transaction)
            .pipe(
                catchError(this.handleError)
            );
    }
}
