import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';

import { of } from "rxjs";
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { MatListModule } from '@angular/material/list';
import { HttpClientModule } from '@angular/common/http'
import { FormsModule } from '@angular/forms';
import {MatPaginatorModule} from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { AppRoutingModule } from './app-routing.module';
import { HackerNewsData, HackerNewsService } from './app.service';

describe('AppComponent', () => {
  beforeEach(async () => {
    
  let mockHackerNewsService: jasmine.SpyObj<HackerNewsService>;

  mockHackerNewsService = jasmine.createSpyObj(["getHackerNewsItems",
     "getSearchResults",
    "getTotalNumberOfItems"]);

  mockHackerNewsService.getHackerNewsItems.and.returnValue(of(items))
  mockHackerNewsService.getSearchResults.and.returnValue(of(items))
  mockHackerNewsService.getTotalNumberOfItems.and.returnValue(of(20))

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
    BrowserModule,
    AppRoutingModule,
    MatListModule,
    HttpClientModule,
    FormsModule,
    MatPaginatorModule,
    BrowserAnimationsModule
      ],
      providers: [{provide: HackerNewsService, useValue: mockHackerNewsService}],
      declarations: [
        AppComponent
      ],
    }).compileComponents();
  });

  var items: HackerNewsData[];
  items = [
    {
      by: "user1",
      descendants: 0,
      id: 1,
      kids: [1, 2, 3],
      score: 5,
      time: 123456,
      title: "Test Title 1",
      type: "story",
      url: "https://stackoverflow.com/"
    },
    {
      by: "user2",
      descendants: 3,
      id: 2,
      kids: [1, 2, 3],
      score: 0,
      time: 12323456,
      title: "Test Title 2",
      type: "none",
      url: "https://google.com/"
    },
    {
      by: "user2",
      descendants: 3,
      id: 2,
      kids: [1, 2, 3],
      score: 0,
      time: 12323456,
      title: "",
      type: "none",
      url: "" 
    }
  ]


  it('should create the app', () => {
    // Arrange
    var fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    fixture.detectChanges();
    
    // Act

    // Assert
    expect(app).toBeTruthy();
  });

  it(`should run the search function and find items`, () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    fixture.detectChanges();

    // Act
    app.search = "Test"
    app.hackerList = JSON.parse(JSON.stringify(items));
    app.runSearch()

    // Assert
    expect(app.hackerList.length).toEqual(3);
  });

  it(`should run the search function and find items`, () => {
    // Arrange
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    fixture.detectChanges();

    // Act
    app.search = "noting"
    app.hackerList = JSON.parse(JSON.stringify(items));
    app.runSearch()

    // Assert
    expect(app.title).toEqual('hacker_news_page');
  });

});
