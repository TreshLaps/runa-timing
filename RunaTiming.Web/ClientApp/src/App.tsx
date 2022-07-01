import * as React from 'react';
import { Route } from 'react-router';
import HomePage from './components/home/HomePage';
import RacePage from './components/race/RacePage';
import Layout from './components/Layout';

export default () => (
  <Layout>
    <Route exact path="/" component={HomePage} />
    <Route exact path="/race/:id" component={RacePage} />
  </Layout>
);
