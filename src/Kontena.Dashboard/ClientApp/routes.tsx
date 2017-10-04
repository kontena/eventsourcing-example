import * as React from 'react';
import { Route, Redirect } from 'react-router-dom';
import { Layout } from './components/Layout';
import { CustomersDashboard } from './components/CustomersDashboard';
import { ProductsDashboard } from './components/ProductsDashboard';

export const routes = <Layout>
    <Redirect from="/" to="/customers"/>
    <Route path='/customers' component={ CustomersDashboard } />
    <Route path='/products' component={ ProductsDashboard } />
</Layout>;
