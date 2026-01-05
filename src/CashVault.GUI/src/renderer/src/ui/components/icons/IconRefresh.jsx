/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconRefresh = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M11.9999 5C14.3557 5 16.4399 6.16276 17.7102 7.95012C18.0301 8.4003 18.6544 8.50588 19.1046 8.18595C19.5548 7.86601 19.6603 7.24171 19.3404 6.79153C17.7111 4.49898 15.0306 3 11.9999 3C7.02932 3 2.99988 7.02944 2.99988 12C2.99988 12.5523 3.44759 13 3.99988 13C4.55216 13 4.99988 12.5523 4.99988 12C4.99988 8.13401 8.13388 5 11.9999 5Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M12 19C9.6442 19 7.55995 17.8372 6.28968 16.0499C5.96975 15.5997 5.34545 15.4941 4.89527 15.8141C4.44509 16.134 4.33951 16.7583 4.65945 17.2085C6.28875 19.501 8.96929 21 12 21C16.9705 21 21 16.9706 21 12C21 11.4477 20.5523 11 20 11C19.4477 11 19 11.4477 19 12C19 15.866 15.866 19 12 19Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M19.7071 8.70711C19.8946 8.51957 20 8.26522 20 8V3C20 2.44772 19.5523 2 19 2C18.4477 2 18 2.44772 18 3V7H14C13.4477 7 13 7.44771 13 8C13 8.55228 13.4477 9 14 9L19 9C19.2652 9 19.5196 8.89464 19.7071 8.70711Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M4.29289 15.2929C4.10536 15.4804 4 15.7348 4 16L4 21C4 21.5523 4.44772 22 5 22C5.55228 22 6 21.5523 6 21V17H10C10.5523 17 11 16.5523 11 16C11 15.4477 10.5523 15 10 15L5 15C4.73478 15 4.48043 15.1054 4.29289 15.2929Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconRefresh
