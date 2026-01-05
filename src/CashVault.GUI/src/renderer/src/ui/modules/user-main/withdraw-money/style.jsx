import styled from '@emotion/styled'

export const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .header-content {
    display: flex;
    gap: 1rem;
  }

  & .alt-text {
    color: var(--secondary-dark);
    font-weight: 600;
    font-size: 1rem;
    line-height: 1.125rem;
    letter-spacing: 4%;
    text-transform: uppercase;
    padding-top: 1rem;
  }

  & .wms-footer {
    padding-top: 1rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
  }

  & .current-balance {
    flex-grow: 100;
  }

  & .current-balance > .text {
    color: var(--secondary-dark);
    font-weight: 500;
    font-size: 1.5rem;
    line-height: 2rem;
    letter-spacing: -3%;
    text-align: right;
  }

  & .current-balance > .value {
    color: black;
    font-weight: 700;
    font-size: 1.5rem;
    line-height: 2rem;
    letter-spacing: -3%;
    text-align: right;
  }

  & .wms-content {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    flex-grow: 100;
  }

  & .separator {
    width: 1.5px;
    height: 2.875rem;
    background-color: var(--primary-medium);
    margin: auto 0;
  }

  & .configuration {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    padding-bottom: 0.75rem;
  }

  & .withdraw-amount {
    display: flex;
    flex-direction: column;
    padding-bottom: 0.75rem;
  }

  & .withdraw-amount .main {
    display: flex;
    gap: 0.25rem;
    align-items: center;

    & .text {
      color: var(--secondary-dark);
      font-weight: 500;
      font-size: 1.5rem;
      line-height: 2rem;
      letter-spacing: -3%;
    }

    & .value {
      flex: 1;
      align-items: center;
      gap: 0.5rem;

      font-weight: 700;
      font-size: 2.125rem;
      line-height: 3.75rem;
      letter-spacing: -4%;
      text-align: right;

      & span {
        padding-left: 0.5rem;
        font-weight: 500;
        font-size: 2.125rem;
        line-height: 2.375;
        letter-spacing: -3%;
        text-align: right;
      }
    }
  }

  & .withdraw-amount .info {
    margin-left: auto;
    display: flex;
    align-items: center;
    gap: 0.5rem;

    & .text {
      color: #6a6a6a;
      font-weight: 600;
      font-size: 1rem;
      line-height: 1.125rem;
      letter-spacing: 4%;
      text-align: right;
      text-transform: uppercase;
    }

    & .value {
      display: flex;
      align-items: center;

      font-weight: 600;
      font-size: 1rem;
      line-height: 1.125rem;
      letter-spacing: 4%;
      text-align: right;
      text-transform: uppercase;

      & span {
        padding-left: 0.5rem;
      }
    }
  }

  & .print-amount {
    display: flex;
    align-items: center;
    gap: 1rem;

    & .input {
      width: 20rem;
    }

    & .value {
      flex: 1;
      margin-left: auto;

      font-weight: 700;
      font-size: 2.125rem;
      line-height: 3.75rem;
      letter-spacing: -4%;
      text-align: right;

      & span {
        padding-left: 0.5rem;
        font-weight: 500;
        font-size: 2.125rem;
        line-height: 2.375;
        letter-spacing: -3%;
        text-align: right;
      }
    }
  }
`
