import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { TransactionService, AuthenticationService, BalanceService } from '@app/services';
import { User, WalletBalance, Transaction } from '@app/models';

@Component({
    selector: 'app-user-info',
    templateUrl: './user-info.component.html'
})
export class UserInfoComponent {
    currentUser: User;
    currentBalance: WalletBalance;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private balanceService: BalanceService,
        private transactionService: TransactionService
    ) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
        this.balanceService.currentBalance.subscribe(x => this.currentBalance = x);
    }

    setExampleTransaction() {
        this.transactionService.setExampleTransaction(new Transaction());
    }
}
