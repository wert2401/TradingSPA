import React, {Component} from "react"

import "./Type.css";
import deleteIcon from"../../icons/Delete.svg"

export default class Type extends Component{
	constructor(props){
		super(props);
		this.toogleActive = this.toogleActive.bind(this);
		this.state = {active: this.props.active};
	}

	toogleActive(){
		var currentState = this.state.active;
		this.setState({active: !currentState});
	}

	render(){
		return(
			<div className={"type" + (this.state.active ? " active" : "")} onClick={this.toogleActive}>
				<div className={"text"}>
					{this.props.text}
				</div>
				<img src={deleteIcon} alt="Delete" />
			</div>
		);
	}
}