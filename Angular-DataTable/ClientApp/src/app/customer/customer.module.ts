import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { RouterModule, Routes } from '@angular/router';

import { OverviewComponent } from './overview/overview.component';
import { ListComponent } from './list/list.component';
import { DetailsComponent } from './details/details.component';

const routes: Routes = [
  { path: 'list', component: OverviewComponent },
  { path: 'details', component: DetailsComponent },
  { path: 'details/:id', component: DetailsComponent }
];


@NgModule({
  imports: [
    CommonModule,
    MatPaginatorModule,
    RouterModule.forChild(routes),
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatInputModule,
    FormsModule
  ],
  declarations: [OverviewComponent, ListComponent, DetailsComponent]
})
export class CustomerModule { }
