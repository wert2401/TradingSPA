import React, { Component } from 'react';
import Layout from './components/Layout';
import ItemList from './components/ItemList';
import TypesList from './components/TypeList';

import './custom.css'

export default class App extends Component {
	constructor(props){
		super(props);
		this.getItems = this.getItems.bind(this);
	}

	getItems() {
		this.setState({items: null});
		//this.render();
		var typesString = "";
		const types = document.getElementsByClassName("active");
		for (let i = 0; i < types.length; i++){
			typesString += types[i].getElementsByClassName("text")[0].innerHTML + ",";
		}
		typesString = typesString.substring(0, typesString.length-1);
		fetch("api/items/" + typesString.toLowerCase())
			.then(response => response.json())
			.then(json => {
				this.setState({ items: json})
			})
			.catch(e => {})
	}

  render () {
    return (
      <Layout>
        <TypesList onClick={this.getItems}/>
        <ItemList items={this.state && this.state.items}/>
      </Layout>
    );
  }
}
