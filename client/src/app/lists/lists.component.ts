import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  
  members: Member[];
  predicate: string = 'liked';
  pagination: Pagination;
  userParams: UserParams;

  constructor(private memberService: MembersService) { 
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getLikes(this.predicate, this.userParams.pageNumber, this.userParams.pageSize).subscribe({
      next: response => {
        console.log(response);
        this.members = response.result,
        this.pagination = response.pagination  
      },
      error: error => console.log(error)
    });
  }

  pageChanged(event: any){
    if(this.userParams.pageNumber != event.page){
      this.userParams.pageNumber = event.page;
      this.loadLikes();
    }
  }

}
