import React, { Component } from 'react';
import './ItemList.css';

export default class ItemList extends Component {
	//Отображение листа Айтемов
	renderList() {
		//Сортировка по убыванию по процентам
		var items = this.props.items;
		items.sort((a, b) => {
			if (((a.steamPrice / a.price) - 1) < ((b.steamPrice / b.price) - 1))
				return 1;

			if (((a.steamPrice / a.price) - 1) > ((b.steamPrice / b.price) - 1))
				return -1;

			return 0;
		})

		//Отрисовка
		return items.map(item => (
			<a key={item.id} href={"https://market.dota2.net/item/" + item.idFirst + "-" + item.idSecond} target="_blank" rel="noopener noreferrer">
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

	//То, что отображается до загрузки предметов
	wait() {
		return (
			<div className="waitText">Загружаю твою выгоду...</div>
		)
	}

	render() {
		return (
			<div className="items-holder">
				{this.props.items ? this.renderList() : this.wait()}
			</div>
		);
	}
}
