import { Component } from '@angular/core';
import { TransactionService } from '@app/services/transaction.service';
import { Transaction, TransactionStatus } from '@app/models';
import { UserService } from '@app/services/user.service';

@Component({
    selector: 'app-transactions-history',
    templateUrl: './transactions-history.component.html'
})
export class TransactionsHistoryComponent {
    public transactions: Transaction[];
    loading = false;
    searchString: string;
    order: string[] = [];
    orderFromEvent: object = {};
    TransactionStatus = TransactionStatus;

    constructor(
        private transactionService: TransactionService,
        private userService: UserService,
    ) {
        this.transactionService.currentTransacions.subscribe(data => {
            for (const el of data) {
                if (el.recipientId !== el.senderId) {

                    if (el.recipientId !== null) {
                        this.userService.getUser(el.recipientId).subscribe(x => el.recipientEmail = x.email);
                    }
                    if (el.senderId !== null) {
                        this.userService.getUser(el.senderId).subscribe(x => el.senderEmail = x.email);
                    }
                } else {
                    if (el.senderId !== null) {
                        this.userService
                            .getUser(el.recipientId)
                            .subscribe(x => {
                                el.recipientEmail = x.email;
                                el.senderEmail = x.email;
                            });
                    }
                }
            }

            this.transactions = data;
        });
    }

    setExampleTransaction(transaction: Transaction) {
        this.transactionService.setExampleTransaction(transaction);
    }

    covertOrder() {
        this.order = [];

        for (const name in this.orderFromEvent) {
            if (this.orderFromEvent[name] === 1) {
                this.order.push(name);
            } else if (this.orderFromEvent[name] === 2) {
                this.order.push('-' + name);
            }
        }
    }

    onHeadChange(event: any) {
        this.orderFromEvent[event.name] = event.state;
        this.covertOrder();
    }
}
