export const updateObject = (oldObject, updatedProperties) => {
  return {
    ...oldObject,
    ...updatedProperties,
  };
};

export const checkValidity = (value, rules) => {
  let rs: any = { isValid: true };
  if (!rules) {
    return rs;
  }

  if (rules.required) {
    rs.isValid = (value || "").trim() !== "";
    if (!rs.isValid) {
      rs.required = true;
      return rs;
    }
  }

  if (rules.minLength) {
    rs.isValid = value.length >= rules.minLength;
    if (!rs.isValid) {
      rs.minLength = true;
      return rs;
    }
  }

  if (rules.maxLength) {
    rs.isValid = value.length <= rules.maxLength;
    if (!rs.isValid) {
      rs.maxLength = true;
      return rs;
    }
  }

  if (rules.isEmail) {
    const pattern = /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/;
    rs.isValid = pattern.test(value);
    if (!rs.isValid) {
      rs.email = true;
      return rs;
    }
  }

  if (rules.isNumeric) {
    const pattern = /^\d+$/;
    rs.isValid = pattern.test(value);
    if (!rs.isValid) {
      rs.numeric = true;
      return rs;
    }
  }

  return rs;
};
