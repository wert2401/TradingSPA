import React, { Component, Fragment } from "react";
import Type from "../Type";

import "./TypeList.css"

export default class TypesList extends Component {
	render() {
		return (
			<Fragment>
				<div className="typesList">
					<Type text="Common" active/>
					<Type text="Uncommon" />
					<Type text="Rare" />
					<Type text="Mythical" />
					<Type text="Legendary" />
					<Type text="Immortal" />
					<Type text="Ancient" />
					<Type text="Arcana" />
				</div>
				<button className="list-btn button" onClick={this.props.onClick}>Обновить предметы</button>
			</Fragment>
		);
	}
}