import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";


export interface HackerNewsData {
  by: string;
  descendants: number;
  id: number;
  kids: number[];
  score: number;
  time: number;
  title: string;
  type: string;
  url: string;
}


@Injectable()
export class HackerNewsService {

  private readonly baseUrl = "https://localhost:44366/api/hackernews/"


  constructor(private readonly http: HttpClient) {

    }

    getHackerNewsItems(page: number): Observable<any> {
      return this.http.get<any>(this.baseUrl + page.toString());
  }

    getSearchResults(searchValue: string): Observable<any> {
      return this.http.get<any>(this.baseUrl + "search/" + searchValue);
  }

  getTotalNumberOfItems(): Observable<number>{
    return this.http.get<any>(this.baseUrl + "count");
  }
}