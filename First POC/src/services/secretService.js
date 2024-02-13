export const SecretService = {
    get: () => {
      return JSON.parse(localStorage.getItem('secrets'));
    },
    set: (secrets) => {
      localStorage.setItem('secrets', JSON.stringify(secrets));
    },
  };
  