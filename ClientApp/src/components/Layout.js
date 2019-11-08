import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu/NavMenu';

export class Layout extends Component {
  render () {
    return (
      <div>
        <Container>
          <NavMenu />
          {this.props.children}
        </Container>
      </div>
    );
  }
}
