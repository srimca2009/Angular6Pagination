import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PaginationService } from './services/pagination.service';
import { CustomerDataService } from './services/customer-data.service';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [],
  providers: [
    PaginationService,
    CustomerDataService
  ],
})
export class CoreModule { }
