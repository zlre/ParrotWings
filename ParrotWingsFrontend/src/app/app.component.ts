import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService, BalanceService, TransactionService } from '@app/services';
import { Transaction, User } from '@app/models';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent  {
    currentUser: User;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private balanceService: BalanceService,
        private transactionService: TransactionService
    ) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    }

    logout() {
        this.authenticationService.logout();
        this.balanceService.logout();
        this.transactionService.logout();
        this.router.navigate(['/login']);
    }

    setExampleTransaction() {
        this.transactionService.setExampleTransaction(new Transaction());
    }
}
