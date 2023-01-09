const check = (pattern: string, toCheck: string) => {
    const regExp = new RegExp(pattern);
    return regExp.test(toCheck);
};

export default { check }