import { TransactionStatus } from '.';

export class Transaction {
    id: string;
    creationDate: string;
    statusDate?: string;
    senderId: string;
    senderEmail: string;
    recipientId: string;
    recipientEmail: string;
    status: TransactionStatus;
    amount: string;
}
