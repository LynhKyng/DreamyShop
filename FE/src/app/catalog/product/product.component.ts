import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { ProductService } from 'src/app/services/product.service';
import { ProductDto } from 'src/app/shared/models/product.dto';
import { DialogService } from 'primeng/dynamicdialog';
import { CreateProductComponent } from './create-product/create-product.component';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent implements OnInit {
  items: MenuItem[] = [];
  activeItem!: MenuItem;
  products: ProductDto[] = [];

  constructor(
    private productService: ProductService,
    private dialogService: DialogService
  ) {}

  ngOnInit() {
    this.items = [
      { label: 'All', icon: 'pi pi-fw pi-home' },
      { label: 'Active', icon: 'pi pi-check-circle' },
      { label: 'In Active', icon: 'pi pi-minus-circle' },
      { label: 'Out of stock', icon: 'pi pi-circle-off' },
      { label: 'On promotion', icon: 'fa fa-bullhorn' },
      { label: 'Hidden', icon: 'fa fa-ban' },
    ];
    this.getAllProducts();
    this.activeItem = this.items[0];
  }

  getAllProducts() {
    this.productService.getProducts().subscribe((data) => {
      this.products = data;
      console.log('data :>> ', this.products);
    });
  }

  showAddModal(): void {
    const ref = this.dialogService.open(CreateProductComponent, {
      header: 'Create new product',
      width: '70%',
    });
  }
}
