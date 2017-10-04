import * as React from 'react';
import { RouteComponentProps, match } from 'react-router';
import 'isomorphic-fetch';
import { Button, Modal } from 'react-bootstrap';

interface CustomerPurchasesProps {
    purchases: CustomerPurchase[];
    show: boolean;
    onClose: () => void;
}

interface PurchaseCustomer {
    id: string;
    firstName: string;
    lastName: string;
}

interface PurchaseProduct {
    id: string;
    name: string;
    price: number;
}

export interface CustomerPurchase {
    customer: PurchaseCustomer;
    product: PurchaseProduct;
    amountSpent: number;
    transactionDate: Date;
}

export class CustomerPurchasesModal extends React.Component<CustomerPurchasesProps, {}> {
    constructor(props: CustomerPurchasesProps) {
        super(props);
    }

    public render() {
        let contents = this.renderTable(this.props.purchases || []);

        return <Modal bsSize='large' show={ this.props.show } onHide={ () => this.props.onClose() }>
                <Modal.Header closeButton>
                    <Modal.Title>Modal heading</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    { contents }
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={() => this.props.onClose() }>Close</Button>
                </Modal.Footer>
            </Modal>;
    }

    private renderTable(purchases: CustomerPurchase[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Customer</th>
                    <th>Product</th>
                    <th>Amount Spent</th>
                    <th>Date</th>
                </tr>
            </thead>
            <tbody>
            {purchases.map(purchase =>
                <tr key={ purchase.customer.id + purchase.product.id }>
                    <td>{ purchase.customer.firstName + ' ' + purchase.customer.lastName }</td>
                    <td>{ purchase.product.name }</td>
                    <td>{ purchase.amountSpent }</td>
                    <td>{ new Date(purchase.transactionDate).toLocaleString() }</td>
                </tr>
            )}
            </tbody>
        </table>;
    }
}


