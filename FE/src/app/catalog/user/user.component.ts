import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { UserService } from 'src/app/services/user.service';
import { RoleTypes } from 'src/app/shared/enums/role-types';
import { UserDisplayDto, UserDto } from 'src/app/shared/models/user-dto';
import { AssignRoleComponent } from './assign-role/assign-role.component';
import { UserCreateComponent } from './user-create/user-create.component';
import { UserEditComponent } from './user-edit/user-edit.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
})
export class UserComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  items: MenuItem[] = [];
  activeItem!: MenuItem;
  users: UserDisplayDto[] = [];
  userReceives: UserDto[] = [];
  constructor(
    private userService: UserService,
    private dialogService: DialogService
  ) {}

  ngOnInit() {
    this.getAllUsers();
  }

  getAllUsers() {
    this.userService
      .getAllUsers(this.maxResultCount, this.currentPage)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: any) => {
          this.userReceives = response.result.items;
          this.convertToUserDisplay(this.userReceives);
          console.log('this.users :>> ', this.users);
        },
        error: () => {},
      });
  }

  convertToUserDisplay(users: UserDto[]) {
    this.users = users.map((userDto) => {
      // Chuyển đổi từng phần tử UserDto sang UserDisplayDto
      return {
        id: userDto.id,
        fullName: userDto.fullName,
        genderType: userDto.genderType,
        dob: userDto.dob,
        avatar: userDto.avatar,
        email: userDto.email,
        phone: userDto.phone,
        address: userDto.address,
        roleTypes: userDto.roleTypes.map((roleType) => RoleTypes[roleType]),
      };
    });
  }

  //PAGING
  totalCounts: number = 0;
  maxResultCount: number = 5;
  currentPage: number = 1;
  //rows: number = this.totalCounts / this.maxResultCount;

  onPageChange(event: any): void {
    this.currentPage = event.page + 1;
    this.maxResultCount = event.rows;
    this.getAllUsers();
  }

  showAssignRole(idUser: number) {
    let user = this.getUserById(idUser);
    this.userService.setUser(user as UserDto);
    const ref = this.dialogService.open(AssignRoleComponent, {
      header: 'CẬP NHẬT QUYỀN',
      width: '20%',
      height: '48%',
    });
    ref.onClose.subscribe((data: any) => {
      if (data) {
        this.getAllUsers();
      }
    });
  }

  getUserById(id: number): UserDto | undefined {
    return this.userReceives.find((user) => user.id === id);
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  createUser() {}

  updateUser(idUser: number) {
    this.userService.setUser(this.getUserById(idUser) as UserDto);
    const ref = this.dialogService.open(UserEditComponent, {
      header: 'CHỈNH SỬA NGƯỜI DÙNG',
      width: '30%',
      height: '75%',
    });
    ref.onClose.subscribe((data: any) => {
      if (data) {
        this.getAllUsers();
      }
    });
  }

  deleteUser() {}
}
