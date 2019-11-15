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
    items.sort((a, b) => {
      if (((a.steamPrice / a.price) - 1) < ((b.steamPrice / b.price) - 1))
        return 1;

      if (((a.steamPrice / a.price) - 1) > ((b.steamPrice / b.price) - 1))
        return -1;
      
      return 0;
    })
    return items.map(item => (
      <a href={"https://market.dota2.net/item/" + item.idFirst + "-" + item.idSecond} target="_blank">
        <div className="item">
          <div className="item__foreground">
            <div className="item__text">Name: {item.name}</div>
            <div className="item__line" style={{ background: "rgba(255, 77, 0, 0.5)", transition: "all 0.7s" }}></div>
            <div className="item__text">Price: {item.price.toFixed(3)}$</div>
            <div className="item__line" style={{ background: "rgba(255, 77, 0, 0.75)", transition: "all 0.85s" }}></div>
            <div className="item__text">Steam price: {item.steamPrice.toFixed(3)}$</div>
            <div className="item__line" style={{ background: "rgba(255, 77, 0, 1)", transition: "all 1s" }}></div>
            <div className="item__text">Value: {(((item.steamPrice / item.price) - 1) * 100).toFixed(2)}%</div>
            <img className="item__image" src={item.imageHref} alt="Item"></img>
          </div>
          <div className="item__background"></div>
        </div>
      </a>
    ))
  }

  wait() {
    return (
      <div className="waitText">Загружаю твою выгоду...</div>
    )
  }

  render() {
    return (
      <div className="items-holder">
        {this.state.state === "ready" ? this.renderList() : this.wait()}
      </div>
    );
  }
}
