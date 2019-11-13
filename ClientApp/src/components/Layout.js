import React, { Component } from 'react';
import { NavMenu } from './NavMenu/NavMenu';

export class Layout extends Component {
  render () {
    return (
      <div className="myContainer">
          <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500&display=swap" rel="stylesheet"></link>
          <NavMenu />
          {this.props.children}
      </div>
    );
  }
}
