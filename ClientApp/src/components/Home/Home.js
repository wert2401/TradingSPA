import React, { Component } from 'react';
import './Home.css'

export class Home extends Component {
  constructor(props) {
    super(props);
    this.state = { items: { "abdc": { name: "JOhn" } }, state: "loads" }
  }

  componentDidMount() {
    this.getItems();
  }

  getItems() {
    fetch("api/items")
      .then(response => response.json())
      .then(json => {
        this.setState({ items: json, state: "ready" })
      })
  }

  renderList() {
    var items = this.state.items;
    return items.map(item => (
      <a href={"https://market.dota2.net/item/" + item.idFirst + "-" + item.idSecond} target="_blank">
        <div className="item">
          <div>Name: {item.name}</div>
          <div>Price: {item.price.toFixed(3)} $</div>
          <div>Price: {item.steamPrice} $</div>
          <div>Good: ?</div>
        </div>
      </a>
    ))
  }

  render() {
    return (
      <div className="items-holder">
        {this.state.state === "ready" ? this.renderList() : "Загружаю твою выгоду..."}
      </div>
    );
  }
}
