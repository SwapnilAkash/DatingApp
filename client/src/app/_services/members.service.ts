import { HttpClient, HttpParams} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members:Member[] = [];

  constructor(private httpClient: HttpClient) { }

  getMembers(userParams: UserParams){
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users',params);
  }

  private getPaginatedResult<T>(url: string,params: HttpParams) {
    
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    return this.httpClient.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }

        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(page: number, itemsPerPage: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);

    return params;
  }

  getMember(username:string){
    const member = this.members.find(x => x.username === username);
    if(member != undefined){
      return of(member);
    }
    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member){
    return this.httpClient.put<Member>(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: Number){
    return this.httpClient.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: Number){
    return this.httpClient.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}
