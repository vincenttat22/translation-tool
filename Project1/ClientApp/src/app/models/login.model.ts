export class Token {
    Value: string = '';
  }

  export class UserProfile {
      id: string;
      firstName: string;
      lastName: string;
      email: string;
      roles: string[];
      lockoutEnabled: string;
      constructor(firstName = '', lastName = '', email = '') {
          this.firstName = firstName;
          this.lastName = lastName;
          this.email = email;
      }
  }