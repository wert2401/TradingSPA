import React, { Component } from 'react';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;c

  render () {
    return (
      <header>
        <h1>Dota Market And Steam</h1>
        <span className="line_1"></span>
        <span className="line_2"></span>
      </header>
    );
  }
}
