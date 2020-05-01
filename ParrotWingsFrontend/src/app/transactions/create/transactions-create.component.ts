import { Component, OnInit } from '@angular/core';
import { TransactionService } from '@app/services/transaction.service';
import { Transaction, User } from '@app/models';
import { UserService } from '@app/services/user.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthenticationService } from '@app/services';

@Component({
    selector: 'app-transactions-create',
    templateUrl: './transactions-create.component.html'
})
export class TransactionsCreateComponent implements OnInit {
    users: User[];
    createForm: FormGroup;
    error = '';
    submitted = false;
    dismiss: string;

    constructor(
        private formBuilder: FormBuilder,
        private transactionService: TransactionService,
        private userService: UserService,
        private authenticationService: AuthenticationService
    ) {
    }

    ngOnInit() {
        this.userService.getUsers().subscribe(x => this.users = x.filter(u => u.id !== this.authenticationService.currentUserValue.id ));

        this.createForm = this.formBuilder.group({
            recipient: ['', [Validators.required]],
            amount: [0, [Validators.required]]
        });

        this.transactionService.exampleTransaction.subscribe(x => {
            if (this.users && this.users.find(u => u.id === x.recipientId )) {
                this.createForm.controls.recipient.setValue(x.recipientId);
            }
            this.createForm.controls.amount.setValue(x.amount);
        });
    }

    get form() { return this.createForm.controls; }

    onAddTransaction() {
        this.submitted = true;

        if (this.createForm.invalid) {
            return;
        }

        const transaction = new Transaction();

        transaction.recipientId = this.form.recipient.value;
        transaction.amount = this.form.amount.value;

        this.dismiss = 'modal';

        this.transactionService.createTransaction(transaction)
            .subscribe(
                data => {
                    this.dismiss = '';
                    this.transactionService.commitTransaction(data).subscribe();
                },
                error => {
                    this.error = error;
                    this.dismiss = '';
                }
            );
    }
}
