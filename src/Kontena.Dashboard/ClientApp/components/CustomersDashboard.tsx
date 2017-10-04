import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { CustomerPurchasesModal, CustomerPurchase } from './CustomerPurchasesModal';
import { Button } from 'react-bootstrap';
import Websocket from 'react-websocket';

interface CustomersState {
    customers: Customer[];
    purchases: CustomerPurchase[];
    loading: boolean;
    showModal: boolean;
    customerId: string;
}

interface Customer {
    id: string;
    firstName: string;
    lastName: string;
    purchaseCount: number;
    totalSpent: number;
}

export class CustomersDashboard extends React.Component<RouteComponentProps<{}>, CustomersState> {
    constructor() {
        super();
        this.state = {
            customers: [],
            purchases: [],
            loading: true,
            showModal: false,
            customerId: ''
        };

        this.fetchData();
    }

    public render() {
        const wsUrl = `ws://${window.location.host}/ws`;

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderTable(this.state.customers);

        return <div>
            <h1>Customers</h1>

            { contents }

            <Websocket url={ wsUrl } onMessage={() => this.fetchData() } />
            <CustomerPurchasesModal purchases={ this.state.purchases }
                                        show={ this.state.showModal }
                                        onClose={ () => this.closeModal() } />
        </div>;
    }

    private fetchData() {
        return fetch('api/customers')
                .then(response => response.json() as Promise<Customer[]>)
                .then(data => {
                    this.setState({ customers: data, loading: false });

                    if (this.state.showModal) {
                        return this.openModal(this.state.customerId);
                    }
                });
    }

    private openModal(id: string) {
        return fetch(`api/customers/${id}/purchases`)
            .then(response => response.json() as Promise<CustomerPurchase[]>)
            .then(data => {
                this.setState({ purchases: data, showModal: true, customerId: id });
            });
    }

    private closeModal() {
        this.setState({ showModal: false });
        return this.fetchData();
    }

    private renderTable(customers: Customer[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Name</th>
                    <th># Purchases</th>
                    <th>Total Spent</th>
                </tr>
            </thead>
            <tbody>
            {customers.map(customer =>
                <tr key={ customer.id }>
                    <td>
                        {
                            customer.purchaseCount > 0 ?
                            (<Button bsStyle="link" onClick={ () => this.openModal(customer.id) }>
                                { customer.firstName + ' ' + customer.lastName }
                            </Button>)
                            : <span className='item-name'>{ customer.firstName + ' ' + customer.lastName }</span>
                        }
                    </td>
                    <td>{ customer.purchaseCount }</td>
                    <td>{ customer.totalSpent }</td>
                </tr>
            )}
            </tbody>
        </table>;
    }
}


