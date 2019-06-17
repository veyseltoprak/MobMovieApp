import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';

class App extends Component {
    state = {  
      isLoading : true,
      data : ''
    }
    //Default Movie
    componentDidMount()
    {
      fetch('api/movie/search/Breaking Bad')
      .then(response => response.json())
      .then(d => this.setState({data: d, isLoading: false}))
    }

    //Input Change 
    updateSearch(searchTerm) {
      fetch('api/movie/search/' + searchTerm)
      .then(response => response.json())
      .then(d => this.setState({data: d, isLoading: false}))
    }

    render()
    {
      const {data,isLoading} = this.state;
      if(isLoading)
      {
        return <p>Loading</p>
        ;
      }
      else if(data == null || data == '' || data.title == null || data.title == '')
      {
        return (
          <div class="container">
            <input class="form-control" type="text" placeholder="Search" aria-label="Search" onChange={ (e) => this.updateSearch(e.target.value) }/>
            </div>
        )
        ;
      }
      else
      {
        return (
          <div class="container">
            <input class="form-control" type="text" placeholder="Search" aria-label="Search" onChange={ (e) => this.updateSearch(e.target.value) }/>
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12">
                    <div class="well well-sm">
                        <div class="row">
                            <div class="col-sm-6 col-md-4">
                                <img src={data.poster} alt="" class="img-rounded img-responsive" />
                            </div>
                            <div class="col-sm-6 col-md-8">
                                <h4>
                                    {data.title}</h4>
                                <small> Year: {data.year} </small>
                                <p>
                                <small> ImdbID: {data.imdbID} </small>
                                <br />
                                <small> Type: {data.type} </small>
                                <br /> 
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        );
      }
    }
}

export default App;
