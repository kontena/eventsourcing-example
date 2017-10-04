import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { CustomerPurchasesModal, CustomerPurchase } from './CustomerPurchasesModal';
import { Button } from 'react-bootstrap';
import Websocket from 'react-websocket';

interface ProductsState {
    products: Product[];
    purchases: CustomerPurchase[];
    loading: boolean;
    showModal: boolean;
    productId: string;
}

interface Product {
    id: string;
    name: string;
    price: number;
    purchaseCount: number;
    customerCount: number;
    totalSpent: number;
}

export class ProductsDashboard extends React.Component<RouteComponentProps<{}>, ProductsState> {
    constructor(props: RouteComponentProps<{}>) {
        super(props);
        this.state = {
            products: [],
            purchases: [],
            loading: true,
            showModal: false,
            productId: ''
        };

        this.fetchData();
    }

    public render() {
        const wsUrl = `ws://${window.location.host}/ws`;

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderTable(this.state.products);

        return <div>
            <h1>Products</h1>

            { contents }

            <Websocket url={ wsUrl } onMessage={() => this.fetchData() } />
            <CustomerPurchasesModal purchases={ this.state.purchases }
                                         show={ this.state.showModal }
                                         onClose={ () => this.closeModal() } />
        </div>;
    }

    private fetchData() {
        return fetch('api/products')
            .then(response => response.json() as Promise<Product[]>)
            .then(data => {
                this.setState({ products: data, loading: false });

                if (this.state.showModal) {
                    return this.openModal(this.state.productId);
                }
            });
    }

    private openModal(id: string) {
        return fetch(`api/products/${id}/purchases`)
            .then(response => response.json() as Promise<CustomerPurchase[]>)
            .then(data => {
                this.setState({ purchases: data, showModal: true, productId: id });
            });
    }

    private closeModal() {
        this.setState({ showModal: false });
        return this.fetchData();
    }

    private renderTable(products: Product[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Price</th>
                    <th># Purchases</th>
                    <th># Customers</th>
                    <th>Total Spent</th>
                </tr>
            </thead>
            <tbody>
            {products.map(product =>
                <tr key={ product.id }>
                    <td>
                        {
                            product.purchaseCount > 0 ?
                            ( <Button bsStyle="link" onClick={ () => this.openModal(product.id) }>
                                { product.name }
                            </Button>)
                            : <span className='item-name'>{ product.name }</span>
                        }

                    </td>
                    <td>{ product.price }</td>
                    <td>{ product.purchaseCount }</td>
                    <td>{ product.customerCount }</td>
                    <td>{ product.totalSpent }</td>
                </tr>
            )}
            </tbody>
        </table>;
    }
}


