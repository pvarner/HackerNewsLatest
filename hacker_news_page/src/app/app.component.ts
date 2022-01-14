import { Component, OnInit } from '@angular/core';
import {PageEvent} from '@angular/material/paginator';

import { HackerNewsData, HackerNewsService } from './app.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'hacker_news_page';
  hackerList: HackerNewsData[];
  search: string;
  showResults: boolean;
  displayText: string;
  hostname: string;
  totalItems: number;
  pageSize: number;
  pageEvent: PageEvent;
  currentPage: number;
  searchResultsDisplaying: boolean;



  constructor(private readonly hackerNewsService: HackerNewsService) {
  this.hackerList = {} as HackerNewsData[];
  this.search = "";
  this.showResults= true;
  this.displayText = "";
  this.hostname = "http://localhost:4200/"
  this.totalItems = 0;
  this.pageSize = 20;
  this.currentPage = 0;
  this.searchResultsDisplaying = false;

  }

  ngOnInit(): void {
    this.displayText = "Loading...";
    this.getTotalNumberOfItems();
    this.getItemPage(0);
  }

// @description: gets the number of items in the hackerNews newest 
getTotalNumberOfItems(){
  this.hackerNewsService.getTotalNumberOfItems().subscribe( resp => {
    this.totalItems = resp;
  }); 
}

// @description: Called when the the pagination is changed 
// @arg event: contains information relevant to paging 
getNewPage(event?:PageEvent){
  if (event != null){
    this.getItemPage(event.pageIndex)
    this.currentPage = event.pageIndex;
  }
}

// @description: gets the page of items given a page number
// @arg page: the number of the requested page
getItemPage(page: number) {
  this.hackerNewsService.getHackerNewsItems(page).subscribe(resp => {
    this.displayText = "";
    this.hackerList = JSON.parse(JSON.stringify(resp));

    //for (let item of resp){
    //let val: HackerNewsData = {
      //by: item.by,
      //descendants: item.descendants,
      //id: item.id,
      //kids: item.kids,
      //score: item.score,
      //time: item.time,
      //title: item.title,
      //type: item.type,
      //url: item.url}
      
      //this.hackerList.push(val);
    //}

    this.searchResultsDisplaying = false;

    // Remove all null returned index's
    this.hackerList = this.hackerList.filter(function (el) {
      return el != null;
    });

    // if the url is ever blank update the url
    for (var item of this.hackerList) {
      if (!item.url) {
        item.url = "https://news.ycombinator.com/item?id=" + item.id.toString();
      }
    }
  })
}

// @description: Performs search based on user request, request stored in variable 'search' 
runSearch(){
  this.showResults = false;
  this.displayText = "Searching through " + this.totalItems + " posts..."
  this.searchResultsDisplaying = true;

  this.hackerNewsService.getSearchResults(this.search).subscribe( resp => {
    this.showResults = true;
    this.hackerList = resp;

    // If nothing found from search display message
    if (this.hackerList.length == 0){
      this.showResults = false;
      this.displayText = "No Results Found!" 
    }
    else{
      this.showResults = true;
      this.displayText = ""; 

    }

    // Removes null values returned from HackerRank response
    this.hackerList = this.hackerList.filter(function (el) {
      return el != null;
    });

    // if the url is ever blank update the url
    for (var item of this.hackerList) {
      if (!item.url) {
        item.url = "https://news.ycombinator.com/item?id=" + item.id.toString();
      }
    }
  },
  (err) => {
    this.displayText = "Something went Wron!"
  })

}

}
